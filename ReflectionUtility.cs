using System;
using System.Reflection;

namespace Claws
{
    internal static class ReflectionUtility
    {
        public static void SetPrivateMember(this object target, string memberName, object value)
        {
            var type = target.GetType();

            FieldInfo targetField;
            if ((targetField = type.GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic)) != null)
            {
                targetField.SetValue(target, value);
                return;
            }

            PropertyInfo targetProperty;
            if ((targetProperty = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                targetProperty.SetValue(target, value);
            }
        }

        public static void SetStaticMember(this Type type, string memberName, object value)
        {
            FieldInfo targetField;
            if ((targetField = type.GetField(memberName, BindingFlags.Static | BindingFlags.NonPublic)) != null)
            {
                targetField.SetValue(null, value);
                return;
            }

            PropertyInfo targetProperty;
            if ((targetProperty = type.GetProperty(memberName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                targetProperty.SetValue(null, value);
            }
        }

        public static T GetPrivateField<T>(this object target, string memberName)
        {
            var type = target.GetType();

            FieldInfo targetField;
            if ((targetField = type.GetField(memberName, BindingFlags.Instance | BindingFlags.NonPublic)) != null)
            {
                return (T)targetField.GetValue(target);
            }

            PropertyInfo targetProperty;
            if ((targetProperty = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null)
            {
                return (T)targetProperty.GetValue(target);
            }

            return default;
        }

        public static void InvokePrivateMethod(this object target, string methodName, object[] methodParams)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(target, methodParams);
        }
    }
}
