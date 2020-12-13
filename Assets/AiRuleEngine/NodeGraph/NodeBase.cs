#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace AiRuleEngine
{
    public enum NodeType
    {
        RELATIONAL,
        LOGICAL,
        ARITHMETIC,
        CONSTANT,
        VARIABLE,
		SETVARIABLE,
		CALLSCRIPT
    }
	
	public abstract class NodeBase
    {
		public NodeType m_NodeType { get; set; }
		public bool m_ShowChildren { get; set; }
		public List<NodeBase> m_Children;
		public Rect m_Window = new Rect(10, 100, 100, 100);
		public NodeContainer m_Container;
		public int m_MaxChildren { get; set; }
		public Rect m_DeleteButton;

		public NodeBase()
		{
			m_DeleteButton = new Rect(90, 10, 10, 10);
			m_Children = new List<NodeBase>();
			m_Container = null;
		}

		public NodeBase(NodeContainer container)
		{
			m_DeleteButton = new Rect(90, 90, 10, 10);
			m_Children = new List<NodeBase>();
			m_Container = container;
		}

		public abstract bool CheckChild (NodeBase child);
		public abstract void OnGui();
		public virtual void OnDestroy() {}
		public void AddChild(NodeBase node) { m_Children.Add(node); }
		public void RemoveChild(NodeBase node) { m_Children.Remove(node);}
        public bool GetChildAt(int index, out NodeBase node)
        {
            node = null;

            if (index >= m_Children.Count)
                return false;

            node = m_Children[index];

            return true;
        }

		public void SetMaxChildren (int max)
		{
			m_MaxChildren = max;
		}

		public bool CheckForCycles(NodeBase parent, NodeBase child)
		{
			if (parent.m_Children.Contains(child))
				return true;

			bool result = false;

			foreach (NodeBase node in parent.m_Children) 
			{
				if (result = CheckForCycles (node, child))
					break;
			}

			return result;
		}
    }
}
#endif