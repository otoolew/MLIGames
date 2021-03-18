//-----------------------------------------------------------------
//  Copyright © 2012 Schell Games, LLC. All Rights Reserved. 
//
//  Author: William Roberts
//  Date:   02/07/2012
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SG.Core
{
    /// <summary>
    /// Provides extension methods for the UnityEngine.GameObject class.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Retrieve the name of a possibly null UnityEngine.Object.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string SafeName(this Object instance)
        {
            return instance ? instance.name : "<null>";
        }

        /// <summary>
        /// Reparents a GameObject to another GameObject and allows an argument that will either
        /// preserve the local transform or preserve the world transform.
        /// </summary>
        /// <param name="instance">Object to reparent.</param>
        /// <param name="parent">The new parent of the object.</param>
        /// <param name="preserveLocalTransform">Preseves the local transform.</param>
        [Obsolete("This method is obsolete. Call GameObject.SetParent instead.")]
        public static void ReparentTo(this GameObject instance, GameObject parent, bool preserveLocalTransform = true)
        {
            SetParent(instance, parent, !preserveLocalTransform);
        }

        /// <summary>
        /// SetParent modifies the parent of a GameObject and optionally allows an argument that will
        /// preserve the local or world transform.
        /// </summary>
        /// <param name="instance">Object to reparent.</param>
        /// <param name="parent"></param>
        /// <param name="preserverWorldTransform"></param>
        public static void SetParent([NotNull] this GameObject instance, [CanBeNull] GameObject parent, bool preserverWorldTransform = true)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            instance.transform.SetParent(parent == null ? null : parent.transform, preserverWorldTransform);
        }

        /// <summary>
        /// Reparent all child GameObjects of this GameObject to another GameObject.
        /// </summary>
        /// <param name="instance">Object whose children should be reparented.</param>
        /// <param name="newParent">Child GameObjects will be reparented to this
        /// GameObject</param>
        /// <param name="preserveLocalTransforms">If true, preserve the local
        /// transform of each child (vs the world transform).</param>
        [Obsolete("This method is obsolete.  Call GameObject.SetChildrenParentTo instead.")]
        public static void ReparentChildrenTo(this GameObject instance, GameObject parent, bool preserveLocalTransforms = true)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);
            
            // go through children backwards so that reparenting doesn't bork loop
            for (int i = instance.transform.childCount - 1; i >= 0; i--)
                instance.transform.GetChild(i).gameObject.SetParent(parent, !preserveLocalTransforms);
        }

        /// <summary>
        /// Sets the parent of all children of this GameObject to another GameObject
        /// </summary>
        /// <param name="instance">Object whose children should be reparented.</param>
        /// <param name="parent">Child GameObjects will be reparented to this GameObject</param>
        /// <param name="preserveWorldTransforms">If true, preserve the world
        /// transform of each child (vs the local transform).</param>
        public static void SetChildrenParentTo(this GameObject instance, GameObject parent, bool preserveWorldTransforms = true)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            for (int i = instance.transform.childCount - 1; i >= 0; i--)
                instance.transform.GetChild(i).SetParent(parent == null ? null : parent.transform, preserveWorldTransforms);
        }

        /// <summary>
        /// Create an empty GameObject with the given name as a child of this
        /// GameObject.
        /// </summary>
        /// <param name="instance">Extension</param>
        /// <param name="childName">The name for the child GameObject to be created.</param>
        public static void CreateEmptyChild(this GameObject instance, string childName)
        {
            GameObject child = new GameObject(childName);
            child.transform.parent = instance.transform;
        }

        /// <summary>
        /// Removes the clone portion of a newly instantiated GameObject's 
        /// name.
        /// </summary>
        /// <param name="instance">Object to clean the name of.</param>
        public static void CleanName(this Object instance)
        {
            instance.name = instance.name.Replace("(Clone)", "");
        }

        /// <summary>
        /// Returns a string representing the full node path.
        /// Useful for printing debug information about the
        /// scene graph hierachy when you need to debug outside
        /// of the unity editor.
        /// </summary>
        /// <param name="instance">Game object to get the path for.</param>
        /// <returns>
        /// Full path including the name of the given game object.
        /// </returns>
        public static string GetNodePath(this GameObject instance)
        {
            string path;
            Transform parent;

            if (instance == null)
                return string.Empty;

            path = instance.name;
            parent = instance.transform.parent;

            while (parent != null)
            {
                path = parent.gameObject.name + "/" + path;
                parent = parent.parent;
            }

            return path.TrimStart('/');
        }

        /// <summary>
        /// Hides the specified game object in the editor hierarchy view.
        /// </summary>
        public static void HideInHierachy(this GameObject instance)
        {
            HideInHierachy(instance, true);
        }

        /// <summary>
        /// Hides or shows the specified game object in the editor hierarchy view.
        /// </summary>
        /// <param name="instance">Instance of game object to hide or show.</param>
        /// <param name="hide">True to hide, false otherwise</param>
        public static void HideInHierachy(this GameObject instance, bool hide)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            instance.hideFlags = hide ? HideFlags.HideInHierarchy : 0;

            // Hack to refresh the hierachy view.
            //@Unity4 - these used to be SetActiveRecursively, not sure if this hack is even still relevant.
            // Also does not look like this is used anywhere in Librariana
            instance.SetActive(false);            
            instance.SetActive(true);
        }

        /// <summary>
        /// Hides or shows the specified game object in the editor hierarchy view.
        /// The game object will never be saved.
        /// </summary>
        /// <param name="instance">Instance of game object to hide or show.</param>
        /// <param name="hide">True to hide, false otherwise.</param>
        public static void DontSaveAndHideInHierachy(this GameObject instance, bool hide)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            instance.hideFlags = hide ? HideFlags.HideAndDontSave : HideFlags.DontSave;

            // Hack to refresh the hierachy view.
            //@Unity4 - these used to be SetActiveRecursively, not sure if this hack is even still relevant.
            // Also does not look like this is used anywhere in Librariana
            instance.SetActive(false);
            instance.SetActive(true);
        }

        /// <summary>
        /// Recursively sets the hide flags for the specified game object and all of it's children
        /// in the editor hierarchy view. 
        /// </summary>
        /// <param name="instance">Instance of game object to set the hide flags on</param>
        /// <param name="hideFlags">The flags to use.</param>
        public static void RecursivelySetFideFlags(this GameObject instance, HideFlags hideFlags)
        {
            GameObject current;
            Stack<GameObject> children = new Stack<GameObject>();

            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            children.Push(instance);

            while (children.Count > 0)
            {
                current = children.Pop();
                current.hideFlags = hideFlags;

                foreach (Transform child in current.transform)
                    children.Push(child.gameObject);
            }
        }

        /// <summary>
        /// Resets the given game objects transformation back to a default
        /// state of position (0,0,0), rotatopm (0,0,0) scale (1,1,1).
        /// </summary>
        /// <param name="instance"></param>
        public static void ResetTransformation(this GameObject instance)
        {
            if (instance == null)
                throw new NullReferenceException(NullReferenceMessage);

            Transform transform = instance.transform;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Returns true if this GameObject has an ancestor with a Rigidbody
        /// Component. Useful for determining if a Collider is static or not.
        /// </summary>
        public static bool HasRigidbodyAncestor( this GameObject instance ) {
            while( instance != null ) {
                if( instance.GetComponent<Rigidbody>() != null ) {
                    return true;
                }
                instance = instance.transform.parent == null ? null : instance.transform.parent.gameObject;
            }
            return false;
        }

        /// <summary>
        /// Finds a game object under this one with the given name. This will 
        /// return the first encountered GameObject with the name in a breadth 
        /// first search of all GameObjects below this one.
        /// </summary>
        /// <param name="go">The GameObject to search under.</param>
        /// <param name="name">The name of the object to search for.</param>
        /// <returns>
        /// A GameObject under this with the name supplied or null if none is 
        /// found.
        /// </returns>
        public static GameObject FindDescendant(this GameObject go, string name)
        {
            Transform result = go.transform.FindDescendant(name);
            if (result != null)
                return result.gameObject;
            return null;
        }

        /// <summary>
        /// Finds a game object with a given name or Find path.
        /// It will search for by path first and if nothing is found will attempt the full
        /// breadth first seach of all GameObjects for a given name.
        /// </summary>
        /// <param name="go">The GameObject to search under.</param>
        /// <param name="findStr">The name of the object to search for.</param>
        /// <returns>
        /// A GameObject under this with the name supplied or null if none is 
        /// found.
        /// </returns>
        public static GameObject FindDescendantWithPathOrName(this GameObject go, string findStr)
        {
            if (string.IsNullOrEmpty(findStr))
                return null;

            Transform foundTrans = go.transform.Find(findStr);

            if (foundTrans == null)
            {
                GameObject childGo = go.FindDescendant(findStr);
                if (childGo != null)
                    foundTrans = childGo.transform;
            }

            return foundTrans == null ? null : foundTrans.gameObject;
        }

        /// <summary>
        /// (Sam) We were creating this queue every time we were doing a FindDescendant query.  Now we have one statically 
        /// defined queue to do this search, seems like a better way to go garbage collection-wise.
        /// </summary>
        private static Queue<Transform> _queue = new Queue<Transform>();

        /// <summary>
        /// Finds a transform under this one with the given name. This will 
        /// return the first encountered transform with the name in a breadth 
        /// first search of all transforms below this one.
        /// </summary>
        /// <param name="t">The transform to search under.</param>
        /// <param name="name">The name of the transform to search for.</param>
        /// <returns>
        /// A transform under this with the name supplied or null if none is 
        /// found.
        /// </returns>
        public static Transform FindDescendant(this Transform t, string name)
        {
            _queue.Clear();
            _queue.Enqueue(t);
            while (_queue.Count > 0)
            {
                Transform check = _queue.Dequeue();
                if (check.name == name)
                    return check;
                foreach (Transform child in check)
                    _queue.Enqueue(child);
            }
            return null;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component{
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Returns the first instance of a given Component type on this
        /// GameObject or any of its children.
        /// </summary>
        /// <param name="instance">Extension</param>
        /// <param name="type">The type of Component to search for.</param>
        /// <param name="includeInactive">If true, the search will include
        /// inactive GameObjects.</param>
        /// <returns>A component of the desired type if one exists, null otherwise.</returns>
        public static Component GetComponentInChildren(this GameObject instance, Type type, bool includeInactive)
        {
            // see if component is on top-level GameObject
            if (includeInactive || instance.activeInHierarchy)
            {
                Component component = instance.GetComponent(type);
                if (component != null)
                    return component;
            }
            // search for component amongst child GameObjects
            if (instance.transform != null)
            {
                foreach (Transform childTransform in instance.transform)
                {
                    Component componentInChild = childTransform.gameObject.GetComponentInChildren(type, includeInactive);
                    if (componentInChild != null)
                        return componentInChild;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the first instance of a given Component type on this
        /// GameObject or any of its children.
        /// </summary>
        /// <param name="instance">Extension</param>
        /// <typeparam name="T">The type of Component to search for.</typeparam>
        /// <param name="includeInactive">If true, the search will include
        /// inactive GameObjects.</param>
        /// <returns>A component of the desired type if one exists, null otherwise.</returns>
        public static T GetComponentInChildren<T>(this GameObject instance, bool includeInactive) where T : Component
        {
            return instance.GetComponentInChildren(typeof(T), includeInactive) as T;
        }

        /// <summary>
        /// Calculate a bounds encapsulating all child renderers.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static Bounds GetBounds(this GameObject root, bool includeInactive = true)
        {
            Bounds bounds = new Bounds(root.transform.position, Vector3.zero);

            foreach (Renderer r in root.GetComponentsInChildren<Renderer>(includeInactive))
            {
                bounds.Encapsulate(r.bounds);
            }
            return bounds;
        }

        /// <summary>
        /// This method will search the entire game object hierarchy for a particular component doing a depth first search.
        /// This method is ideally used for prefabs and not instantiated game objects.
        /// </summary>
        /// <param name="gameObject">Extension</param>
        /// <param name="strComponent"></param>
        /// <returns></returns>
        public static Component FindComponentInPrefab(this GameObject gameObject, string strComponent) 
        {
            Component comp = gameObject.GetComponent(strComponent);
            if (comp != null)
            {
                return comp;
            }

            for (int nCur = 0; nCur < gameObject.transform.childCount; ++nCur)
            {
                Transform child = gameObject.transform.GetChild(nCur);
                comp = FindComponentInPrefab(child.gameObject,strComponent);
                if (comp != null)
                    return comp;
            }
            return null;
        }

        /// <summary>
        /// This method will search the entire game object hierarchy for a particular component doing a depth first search.
        /// This method is ideally used for prefabs and not instantiated game objects.
        /// </summary>
        /// <param name="gameObject">Extension</param>
        /// <param name="strComponent"></param>
        /// <returns></returns>
        public static List<Component> FindComponentsInPrefab(this GameObject gameObject, string strComponent) 
        {
            List<Component> compList = new List<Component>();
            _FindComponentsInPrefab(gameObject, strComponent, compList);
            return compList;
        }

        public static void _FindComponentsInPrefab(this GameObject gameObject, string strComponent, List<Component> compList)
        {
            Component comp = gameObject.GetComponent(strComponent);
            if (comp != null)
                compList.Add(comp);

            for (int nCur = 0; nCur < gameObject.transform.childCount; ++nCur)
            {
                Transform child = gameObject.transform.GetChild(nCur);
                _FindComponentsInPrefab(child.gameObject,strComponent,compList);
            }
        }

        /// <summary>
        /// This method will recursively search for a unhooked monobehavior.
        /// </summary>
        /// <param name="gameObject">Extension</param>
        /// <returns></returns>
        public static bool RecurseAndCheckForUnhookedScript(this GameObject gameObject)
        {
            MonoBehaviour[] mbArr = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in mbArr)
            {
                if (mb == null)
                    return true;
            }
            int nCount = gameObject.transform.childCount;
            for (int nCur = 0; nCur < nCount; ++nCur)
            {
                RecurseAndCheckForUnhookedScript(gameObject.transform.GetChild(nCur).gameObject);
            }
            return false;
        }

        /// <summary>
        /// This method will search for a unhooked monobehavior.
        /// </summary>
        /// <param name="gameObject">Extension</param>
        /// <returns></returns>
        public static bool CheckForUnhookedScript(this GameObject gameObject)
        {
            MonoBehaviour[] mbArr = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in mbArr)
            {
                if (mb == null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Search the hierarchy ancestors for a specific component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetComponentInAncestor<T>(this GameObject gameObject) where T : Component
        {
            T foundComponent;
            Transform transToSearch = gameObject.transform;
            do
            {
                foundComponent = transToSearch.GetComponent<T>();

                if (foundComponent != null)
                    break;

                transToSearch = transToSearch.parent;
            }
            while (transToSearch != null);

            return foundComponent;
        }

        private const string NullReferenceMessage = "Instance must not be a null reference!";
    }
}
