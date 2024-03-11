using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.DAL.Helpers
{
    public class ConvertDataRowToEntities<T>
        where T : class, new()
    {
        public T DataRowToEntitie(DataRow row)
        {
            T obj = new T();

            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                    propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                }
                catch
                {
                    continue;
                }
            }

            return obj;
        }
    }
}
