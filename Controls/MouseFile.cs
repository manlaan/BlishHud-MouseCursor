using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.MouseCursor.Controls
{
    class MouseFile : IEquatable<MouseFile>, IComparable<MouseFile>
    {
        public string Name { get; set; }
        public string File { get; set; }

        public MouseFile() {
            Name = "";
            File = "";
        }

        public override string ToString() {
            return Name;
        }
        public override bool Equals(object obj) {
            if (obj == null) return false;
            MouseFile objAsCls = obj as MouseFile;
            if (objAsCls == null) return false;
            else return Equals(objAsCls);
        }
        public bool Equals(MouseFile other) {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        public override int GetHashCode() {
            return 0;
        }
        public int SortByNameAscending(string name1, string name2) {
            return name1.CompareTo(name2);
        }
        public int CompareTo(MouseFile compare) {
            if (compare == null)
                return 1;
            else
                return this.Name.CompareTo(compare.Name);
        }
    }
}
