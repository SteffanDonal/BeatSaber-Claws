using System;
using System.Reflection;

namespace Claws
{
    internal static class ReflectionUtility
    {
        public static void SetPrivateField(this object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        public static T GetPrivateField<T>(this object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(target);
            return (T)value;
        }

        public static object GetPrivateField(Type targetType, object targetInstance, string fieldName)
        {
            var field = targetInstance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            var value = field.GetValue(targetInstance);
            return value;
        }

        public static void InvokePrivateMethod(this object target, string methodName, object[] methodParams)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(target, methodParams);
        }
    }
}
