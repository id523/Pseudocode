using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudocodeRevisited
{
    public sealed class Library
    {
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
            public override bool Equals(object obj)
            {
                return (obj is NamedObject) && Name.Equals(((NamedObject)obj).Name);
            }
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }
        private Dictionary<string, HashSet<NamedObject>> Groups =
            new Dictionary<string, HashSet<NamedObject>>();
        private void AddSingle(string fullName, NamedObject v)
        {
            HashSet<NamedObject> group;
            if (!Groups.TryGetValue(fullName, out group))
            {
                group = new HashSet<NamedObject>();
                Groups.Add(fullName, group);
            }
            group.Add(v);
        }
        public void AddFunction(string fullName, string valName, PseudocodeFunction val)
        {
            Add(fullName, valName, val);
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
