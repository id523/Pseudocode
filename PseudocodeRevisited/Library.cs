using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudocodeRevisited
{
    public sealed class Library
    {
        public static void Test()
        {
            Library l = new Library();
            l.Add("AB.CD.EF", "val", 25);
            return;
        }
        private struct NamedObject
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public NamedObject(string n, object v)
            {
                Name = n;
                Value = v;
            }
            public void AddTo(ExecutionState s)
            {
                s.Vars.SetVariable(Name, Value);
            }
        }
        private Dictionary<string, List<NamedObject>> Groups =
            new Dictionary<string, List<NamedObject>>();
        private void AddSingle(string fullName, NamedObject v)
        {
            List<NamedObject> group;
            if (!Groups.TryGetValue(fullName, out group))
            {
                group = new List<NamedObject>();
                Groups.Add(fullName, group);
            }
            group.Add(v);
        }
        public void Add(string fullName, string valName, object val)
        {
            NamedObject obj = new NamedObject(valName, val);
            string[] idparts = fullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            AddSingle("", obj);
            if (idparts.Length <= 0) return;
            AddSingle(idparts[0], obj);
            for (int i = 1; i < idparts.Length; i++)
            {
                idparts[i] = idparts[i - 1] + "." + idparts[i];
                AddSingle(idparts[i], obj);
            }
        }
        public void Import(string fullName, ExecutionState s)
        {
            foreach (NamedObject obj in Groups[fullName])
            {
                obj.AddTo(s);
            }  
        }
        public bool CanImport(string id)
        {
            return Groups.ContainsKey(id);
        }
    }
}
