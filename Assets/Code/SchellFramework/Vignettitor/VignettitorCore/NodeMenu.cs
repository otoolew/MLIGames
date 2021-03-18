//------------------------------------------------------------------------------
// Copyright © 2014 Schell Games, LLC. All Rights Reserved.
//
// Contact: Tim Sweeney
//
// Created: Oct 2014
//------------------------------------------------------------------------------

using SG.Core.Collections;
using SG.Vignettitor.VignetteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SG.Vignettitor.NodeViews;
using UnityEngine;

namespace SG.Vignettitor.VignettitorCore
{
    public class NodeMenu
    {
        public class NodeMenuEntry
        {
            public int priority;
            public string nodeName;
            public Texture nodeIcon;
            public Type nodeType;

            public override bool Equals(object obj)
            {
                NodeMenuEntry entry = obj as NodeMenuEntry;
                if (entry != null)
                    return nodeName.Equals(entry.nodeName);
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                if (nodeName == null)
                    return base.GetHashCode();
                return nodeName.GetHashCode();
            }
        }

        public class SubMenu
        {
            private static readonly string[] baseMenu = { "*" };
            public string name;
            public List<SubMenu> subMenus;
            public List<NodeMenuEntry> entries;

            public string[] GetMenusForToolbar()
            {
                return baseMenu.Concat(subMenus.Select(x => x.name)).ToArray();
            }

            public IEnumerable<NodeMenuEntry> GetDistinctEntries()
            {
                return entries.Concat(subMenus.SelectMany(x => x.GetDistinctEntries())).Distinct().OrderByDescending(x => x.priority).ThenBy(x => x.nodeName);
            }

            public IEnumerable<NodeMenuEntry> GetAllEntries()
            {
                return entries.Concat(subMenus.SelectMany(x => x.GetAllEntries()));
            }
        }

        public SubMenu Root = new SubMenu()
        {
            name = string.Empty,
            subMenus = new List<SubMenu>(),

            entries = new List<NodeMenuEntry>()
        };

        protected Type vignetteGraphType;

        protected const int MAX_RECENTS = 5;

        protected virtual string RecentsPrefsKey
        { get { return "Recents_NodeMenu_" + vignetteGraphType; } }

        public EditorRecents<NodeMenuEntry> Recents;

        public NodeMenu(Type vignetteGraphType, bool inheritNodes)
        {
            this.vignetteGraphType = vignetteGraphType;
            List<NodeMenuAttribute> allAttribs = new List<NodeMenuAttribute>();
            Dictionary<NodeMenuAttribute, Type> attribsToTypes = new Dictionary<NodeMenuAttribute, Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] vntypes = assemblies[i].GetTypes();
                for (int t = 0; t < vntypes.Length; t++)
                {
                    NodeMenuAttribute[] menuAttribs = GetAttributesForType(vntypes[t]);
                    for (int a = 0; a < menuAttribs.Length; a++)
                    {
                        if (inheritNodes)
                        {
                            if (menuAttribs[a].VignetteGraphType.IsAssignableFrom(vignetteGraphType))
                            {
                                allAttribs.Add(menuAttribs[a]);
                                attribsToTypes[menuAttribs[a]] = vntypes[t];
                            }
                        }
                        else
                        {
                            if (menuAttribs[a].VignetteGraphType == vignetteGraphType)
                            {
                                allAttribs.Add(menuAttribs[a]);
                                attribsToTypes[menuAttribs[a]] = vntypes[t];
                            }
                        }
                    }
                }
            }
            //allAttribs = allAttribs.OrderByDescending((x) => 0).ThenBy(x => x.menuPath).ToList();

            foreach (NodeMenuAttribute currentAttrib in allAttribs)
            {
                string menuPath = currentAttrib.MenuPath;
                string[] subPaths = menuPath.Split('/');
                SubMenu insertionPoint = Root;
                for (int i = 0; i < subPaths.Length - 1; i++) // note that we stop one before the end
                {
                    string subMenuName = subPaths[i];

                    SubMenu nextSubMenu = insertionPoint.subMenus.Where(x => x.name == subMenuName).FirstOrDefault();
                    if (nextSubMenu == null)
                    {
                        nextSubMenu = new SubMenu()
                        {
                            name = subMenuName,
                            subMenus = new List<SubMenu>(),
                            entries = new List<NodeMenuEntry>()
                        };

                        insertionPoint.subMenus.Add(nextSubMenu);
                    }
                    insertionPoint = nextSubMenu;
                }

                insertionPoint.entries.Add(new NodeMenuEntry()
                {
                    nodeName = subPaths[subPaths.Length - 1],
                    nodeIcon = NodeIconAttribute.GetNodeIcon(attribsToTypes[currentAttrib]),
                    nodeType = attribsToTypes[currentAttrib],
                    priority = currentAttrib.Priority
                });
            }

            Recents = new EditorRecents<NodeMenuEntry>(MAX_RECENTS, RecentsPrefsKey, x => x.nodeName, GetEntryFromName);
        }

        protected virtual NodeMenuAttribute[] GetAttributesForType(Type t)
        {
            if (t.IsSubclassOf(typeof(VignetteNode)))
            {
                List<NodeMenuAttribute> result = new List<NodeMenuAttribute>();
                Attribute[] menuAttribs = Attribute.GetCustomAttributes(t, typeof(NodeMenuAttribute));
                for (int a = 0; a < menuAttribs.Length; a++)
                {
                    if (!menuAttribs[a].GetType().IsSubclassOf(typeof(NodeMenuAttribute)))
                        result.Add(menuAttribs[a] as NodeMenuAttribute);
                }
                return result.ToArray();
            }
            return new NodeMenuAttribute[0];
        }

        protected NodeMenuEntry GetEntryFromName(string name)
        {
            return Root.GetAllEntries().Where(x => x.nodeName == name).FirstOrDefault();
        }
    }
}
