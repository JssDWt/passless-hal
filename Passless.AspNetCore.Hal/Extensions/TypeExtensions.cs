using System;
namespace Passless.AspNetCore.Hal.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is or inherits from the 
        /// specified generic basetype.
        /// </summary>
        /// <returns><c>true</c>, if the type inherits the generic type, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="genericType">Generic type.</param>
        public static bool IsOfGenericType(this Type type, Type genericType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (genericType == null)
            {
                throw new ArgumentNullException(nameof(genericType));
            }

            if (!genericType.IsGenericType)
            {
                throw new ArgumentException(
                    "Parameter is not a generic type.", 
                    nameof(genericType));
            }

            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Gets the generic arguments for the specified generic basetype of
        /// the current type.
        /// </summary>
        /// <returns>The base class's generic type arguments.</returns>
        /// <param name="type">Type.</param>
        /// <param name="genericType">Generic type.</param>
        public static Type[] GetGenericArguments(this Type type, Type genericType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (genericType == null)
            {
                throw new ArgumentNullException(nameof(genericType));
            }

            if (!genericType.IsGenericType)
            {
                throw new ArgumentException(
                    "Parameter is not a generic type.",
                    nameof(genericType));
            }

            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return type.GetGenericArguments();
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
