//-----------------------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: Ryan Hipple
//  Date:   01/14/2013
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SG.Vignettitor.Graph;
using UnityEngine;

namespace SG.Vignettitor.VignetteData
{
    /// <summary>
    /// Vignette Nodes are the elements that make up a vignette. A vignette is
    /// a graph of these nodes with a single VignetteNode in control at any 
    /// given time. Vignette Nodes can perform actions when they become active,
    /// inactive, or at any point when they are active. 
    /// 
    /// Each VignetteNode has a set of children that it can pass control to 
    /// once it has finished execution. Data can also be passed from the one 
    /// node to the next.
    /// </summary>
    [Serializable]
    public abstract class VignetteNode : ScriptableObject
    {
        //protected static Notify Log = Notify.GetInstance(typeof(VignetteNode));

        #region -- Constants --------------------------------------------------
        public const string CHILD_MISMATCH_ERROR =
            "The actual number of children ({0}) does not match the expected number of children ({1}).";
        #endregion -- Constants -----------------------------------------------

        #region -- Public Properties ------------------------------------------
        /// <summary>
        /// Specifies how many outputs a node is expected to have.
        /// </summary>
        public abstract OutputRule OutputRule { get; }

        /// <summary>
        /// Whether or not this node type performs any actions that take any 
        /// noticeable time (that occurrs over multiple frames). All nodes 
        /// that perform actions and exit immediately should return false.
        /// </summary>
        public virtual bool TakesTime { get { return false; } }
        #endregion -- Public Properties ---------------------------------------

        #region -- Public Variables -------------------------------------------
        /// <summary>
        /// An identifier for a node within a vignette graph.
        /// </summary>
        [HideInInspector]
        public int NodeID;

        /// <summary>
        /// The child VignetteNodes of this. Each of these is a candidate to be
        /// the next active VignetteNode.
        /// </summary>
        public VignetteNode[] Children;
        #endregion -- Public Variables ----------------------------------------

        #region -- Validation -------------------------------------------------

        public virtual ContentValidation Validate()
        {
            ContentValidation validation = new ContentValidation();
            if (!ValidateChildrenCount())
                validation.Error(this, "Node {0} has an invalid child count.", this);

            if (!ValidateNullChildren())
                validation.Error(this, "Node {0} has null entries in the child list.", this);

            return validation;
        }

        /// <summary>
        /// Check that the number of children this VignetteNode has makes sense
        /// given its properties.
        /// </summary>
        /// <returns>true if the number of children is correct. false otherwise.</returns>
        public virtual bool ValidateChildrenCount()
        {
            switch (OutputRule.Rule)
            {
                case OutputRule.RuleType.Passthrough:
                    return Children.Length <= 1;
                case OutputRule.RuleType.Static:
                    return Children.Length == OutputRule.Value;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Checks to see if any child nodes are null, which is invalid.
        /// </summary>
        /// <returns>False if any children are null.</returns>
        public virtual bool ValidateNullChildren()
        {
            for (int i = 0; i < Children.Length; i++)
                if (Children[i] == null)
                    return false;
            return true;
        }
        #endregion -- Validation ----------------------------------------------
        
        #region -- Lookahead --------------------------------------------------
        /// <summary>
        /// Check if node supports that particular type of lookahead. Otherwise
        ///  uncertain lookahead results could occur.
        /// </summary>
        /// <typeparam name="T">Type to check for</typeparam>
        /// <returns>
        /// True if this node can look ahead in the grpah for the type.
        /// </returns>
        public virtual bool SupportsLookahead<T>() where T : VignetteNode
        {
            return false;
        }

        /// <summary>
        /// Validate that all possible lookahead paths only reach
        /// SupportsLookahead nodes before reaching a destination node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validation"></param>
        /// <param name="visitedNodes"></param>
        /// <param name="mustFindNode"></param>
        /// <returns></returns>
        public bool ValidateLookahead<T>(ContentValidation validation, HashSet<VignetteNode> visitedNodes, bool mustFindNode) where T : VignetteNode
        {
            bool foundNode = false;

            if (!SupportsLookahead<T>())
            {
                if(mustFindNode)
                    validation.Error(this, "{0} Lookahead not supported for {1} node", typeof(T).Name, GetType().Name);
            }
            else if (typeof(T).IsAssignableFrom(GetType())) // if I the type we're looking for
            {
                foundNode = true;
            }
            else
            {
                if (Children.Length == 0)
                {
                    // no where to go - we are borked
                    if(mustFindNode)
                        validation.Error(this, "{0} Lookahead not found before end of graph", typeof(T).Name);
                }
                else
                {
                    if (visitedNodes.Contains(this))
                    {
                        // we hit a loop in the Validation - we are borked
                        validation.Error(this, "Loop detected in {0} Lookahead, additional errors may be present but stopped checking", typeof(T).Name);
                        return false;
                    }
                    else
                    {
                        visitedNodes.Add(this);
                    }

                    bool allChildrenFoundNode = true;
                    bool anyChildrenFoundNode = false;

                    for(int i = 0; i < Children.Length; i++)
                    {
                        if (Children[i] != null)
                        {
                            bool childFoundNode = Children[i].ValidateLookahead<T>(validation, visitedNodes, mustFindNode);
                            allChildrenFoundNode &= childFoundNode;
                            anyChildrenFoundNode |= childFoundNode;
                        }
                    }

                    if (!allChildrenFoundNode && anyChildrenFoundNode)
                    {
                        validation.Error(this, "Some children {0} Lookahead terminate, but not all", typeof(T).Name);
                    }

                    foundNode = allChildrenFoundNode;
                }
            }

            return foundNode;
        }

        /// <summary>
        /// Returns a list of nodes of type T that are immediate children
        /// of this node.
        /// </summary>
        /// <typeparam name="T">Typeof of VignetteNode to collect</typeparam>
        /// <returns>
        /// List of type T nodes or an empty List if none are found.
        /// </returns>
        public List<T> GetAllChildrenOfType<T>() where T : VignetteNode
        {
            List<T> results = new List<T>();

            if (Children == null)
                return results;

            for (int i = 0; i < Children.Length; i++)
            {
                T current = Children[i] as T;
                if (current != null)
                {
                    results.Add(current);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns a list of nodes of type T that can possibly be executed
        /// after this node is executed. This list may include nodes that are 
        /// exeuected prior to this node if the graph loops back.
        /// </summary>
        /// <typeparam name="T">Type of node to search for.</typeparam>
        /// <returns>
        /// List of type T nodes.
        /// </returns>
        public List<T> GetAllChildrenOfTypeRecursively<T>() where T : VignetteNode
        {
            List<T> results = new List<T>();
            HashSet<VignetteNode> visited = new HashSet<VignetteNode>();
            Stack<VignetteNode> stack = new Stack<VignetteNode>();

            stack.Push(this);

            while (stack.Count > 0)
            {
                VignetteNode current = stack.Pop();

                if (visited.Contains(current))
                    continue;

                T item = current as T;
                if (item != null)
                    results.Add(item);

                visited.Add(current);

                if (current.Children == null)
                    continue;

                for (int i = 0; i < current.Children.Length; i++)
                {
                    if (current.Children[i] != null)
                        stack.Push(current.Children[i]);
                }
            }

            return results;
        }
        #endregion -- Lookahead -----------------------------------------------

        public override string ToString()
        {
            return name + " (ID:" + NodeID + ")";
        }
    }
}
