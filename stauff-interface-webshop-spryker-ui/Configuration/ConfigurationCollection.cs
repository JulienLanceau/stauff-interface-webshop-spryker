using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace stauff_interface_webshop_spryker_ui.Configuration {
    public class ConfigurationCollection<T> : CollectionBase {
        public T this[int index] {
            get { return (T)List[index]; }
        }
        public void Add(T emp) {
            List.Add(emp);
        }
        public void Remove(T emp) {
            List.Remove(emp);
        }
        public void Set(int index, T emp) {
            List[index] = emp;
        }
        protected virtual Type GetItemType(IList coll) {
            System.Reflection.PropertyInfo pi = coll.GetType().GetProperty("Item",
                                                   new Type[] { typeof(T) });
            return pi.PropertyType;
        }
    }
}
