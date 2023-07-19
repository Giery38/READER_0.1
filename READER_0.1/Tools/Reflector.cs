using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Tools
{
    public class Reflector
    {
        string myNamespace;
        Assembly myAssembly;
        public Reflector(string assemblyName, string namespaceName)
       {
            myNamespace = namespaceName;
            myAssembly = null;
            var alist = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (AssemblyName aN in alist)
            {
                if (aN.FullName.StartsWith(assemblyName))
                {
                    myAssembly = Assembly.Load(aN);
                    break;
                }
            }
        }
        public Type GetType(string typeName)
        {
            Type type = null;
            type = myAssembly.GetType(myNamespace + "." + typeName);          
            return type;
        }

        public object Call(Type type, object obj, string func, object[] parameters)
        {           
            MethodInfo methInfo = type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#pragma warning disable CS0168 // Переменная объявлена, но не используется
            try
            {
                if (methInfo != null)
                {
                    ParameterInfo[] parameters1 = methInfo.GetParameters();
                    return methInfo.Invoke(obj, parameters);
                }                     
            }
            catch (Exception ex)
            {

            }           
#pragma warning restore CS0168 // Переменная объявлена, но не используется
            return null;
        }

        public object GetProperty(Type type, object obj, string propertName, object[] parameters)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);           
            if (propertyInfo != null)
            {
                return propertyInfo?.GetValue(obj);
            }
            return null;
        }
        public void SetProperty(Type type, object obj, string propertName, object[] parameters)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (propertyInfo != null)
            {
                propertyInfo?.SetValue(obj, parameters[0]);
            }
        }

        public object GetField(Type type, object obj, string name)
        {       
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var field  = fields.FirstOrDefault(item => item.Name == name);
            return field.GetValue(obj);
        }
    }
}
