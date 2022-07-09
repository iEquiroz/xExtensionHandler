using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace xExtennsionHandlerCore
{
    public static class DataExtension
    {
        static readonly Regex regularExpressionEmail = new Regex(@"^((([a-z]|\d|[!#\$%&'\\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+))|((\x22)((((\x20|\x09)(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))(((\x20|\x09)(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.||~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.||~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");
        public static T ValidateDataRowKey<T>(this T value, DataRow dr, String key)
        {
            try
            {
                if (dr != null && dr.Table.Columns.Contains(key))
                {
                    return dr[key] is DBNull
                        ? (T)Convert.ChangeType(value, typeof(T))
                        : (T)Convert.ChangeType(Convert.ToString(dr[key], CultureInfo.InvariantCulture), typeof(T), CultureInfo.InvariantCulture);
                }
                else { return (T)Convert.ChangeType(value, typeof(T)); }
            }
            catch (Exception) { return (T)Convert.ChangeType(value, typeof(T)); }

        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
       
        public static Boolean IsValidEmail(String email)
        {
            return regularExpressionEmail.IsMatch(email.ToLower().Trim());
        }

        public static string SerializeToXml(Object Obj, Type ObjType)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            XmlSerializer serializer = new XmlSerializer(ObjType);
            serializer.Serialize(stringWriter, Obj);
            return stringBuilder.ToString();
        }
        public static String ReplaceCardNumberCvv(this String paramRequest, String cardNumber, String sCvv = "NoCvv")
        {
            return paramRequest.Replace(cardNumber, "****" + cardNumber.Substring(cardNumber.Length - 4)).Replace(sCvv, "**");
        }
    }
}
