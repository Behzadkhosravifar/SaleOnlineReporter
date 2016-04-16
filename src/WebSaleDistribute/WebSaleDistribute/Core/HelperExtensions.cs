﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AdoManager;

namespace WebSaleDistribute.Core
{
    public static class HelperExtensions
    {
        public static ConnectionManager UsersManagementsDb => AdoManager.ConnectionManager.Find("UsersManagements");

        public static string GetMd5Bytes(this string input)
        {
            byte[] result;
            var buffer = Encoding.UTF8.GetBytes(input);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                result = md5.ComputeHash(buffer);
            }

            return result.ToHex(false);
        }

        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }


        public static dynamic DapperRowToExpando(this object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return (ExpandoObject) expando;
        }

        public static string ReadResourceFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.{path}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
