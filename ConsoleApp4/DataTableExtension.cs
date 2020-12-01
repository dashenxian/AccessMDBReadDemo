using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ConsoleApp4
{
    public static class DataTableExtension
    {
        /// <summary>
        /// dataTable转list
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dt">数据</param>
        /// <returns></returns>
        public static IEnumerable<T> ToList<T>(this DataTable dt) where T : new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<T>();
            }
            var list = new List<T>(dt.Rows.Count);
            var props = typeof(T).GetProperties();

            foreach (DataRow row in dt.Rows)
            {
                var obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in dt.Columns)
                {
                    foreach (var propertyInfo in props)
                    {
                        if (propertyInfo.Name.Equals(column.ColumnName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var propType = propertyInfo.PropertyType;
                            if (row[column] != DBNull.Value)
                            {
                                var value = row[column];
                                if (column.DataType.Name != propertyInfo.PropertyType.Name
                                    && column.DataType.Name != propertyInfo.PropertyType.GetGenericArguments()[0].Name)
                                {
                                    value = ConvertType(propertyInfo.PropertyType, value);
                                }
                                propertyInfo.SetValue(obj, value);
                            }
                        }
                    }
                }
                list.Add(obj);
            }

            return list;
        }
        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="val">要转换的值</param>
        /// <returns></returns>
        public static object ConvertType(Type type, object val)
        {
            //泛型Nullable判断，取其中的类型
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            //string直接返回转换
            if (type.Name.ToLower() == "string")
            {
                return val + "";
            }
            //反射获取TryParse方法
            var tryParse = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                new Type[] { typeof(string), type.MakeByRefType() },
                new ParameterModifier[] { new ParameterModifier(2) });
            var parameters = new object[] { val + "", Activator.CreateInstance(type) };
            var success = (bool?)tryParse?.Invoke(null, parameters);
            //成功返回转换后的值，否则返回原值
            return success == true ? parameters[1] : val;
        }
        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的基础类型</typeparam>
        /// <param name="val">要转换的值</param>
        /// <returns></returns>
        public static T ConvertType<T>(object val)
        {
            if (val == null) return default(T);//返回类型的默认值
            Type tp = typeof(T);
            //泛型Nullable判断，取其中的类型
            if (tp.IsGenericType)
            {
                tp = tp.GetGenericArguments()[0];
            }
            //string直接返回转换
            if (tp.Name.ToLower() == "string")
            {
                return (T)val;
            }
            //反射获取TryParse方法
            var tryParse = tp.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                new Type[] { typeof(string), tp.MakeByRefType() },
                new ParameterModifier[] { new ParameterModifier(2) });
            var parameters = new object[] { val + "", Activator.CreateInstance(tp) };
            var success = (bool?)tryParse?.Invoke(null, parameters);
            //成功返回转换后的值，否则返回类型的默认值
            if (success == true)
            {
                return (T)parameters[1];
            }
            return default(T);
        }

    }
    public static class DataTableExtension1
    {
        /// <summary>
        /// DataTable生成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToList1<T>(this DataTable dataTable) where T : class, new()
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            List<T> collection = new List<T>(dataTable.Rows.Count);
            if (dataTable.Rows.Count == 0)
            {
                return collection;
            }
            Func<DataRow, T> func = ToExpression<T>(dataTable.Rows[0]);

            foreach (DataRow dr in dataTable.Rows)
            {
                collection.Add(func(dr));
            }
            return collection;
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static Func<DataRow, T> ToExpression<T>(DataRow dataRow) where T : class, new()
        {
            if (dataRow == null) throw new ArgumentNullException("dataRow", "当前对象为null 无法转换成实体");
            ParameterExpression paramter = Expression.Parameter(typeof(DataRow), "dr");
            List<MemberBinding> binds = new List<MemberBinding>();
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                String colName = dataRow.Table.Columns[i].ColumnName;
                PropertyInfo pInfo = typeof(T).GetProperty(colName);
                if (pInfo == null) continue;
                MethodInfo mInfo = typeof(DataRowExtensions)
                    .GetMethod("Field",
                        new Type[] { typeof(DataRow), typeof(String) }
                        )
                    .MakeGenericMethod(pInfo.PropertyType);
                MethodCallExpression call = Expression.Call(mInfo, paramter, Expression.Constant(colName, typeof(String)));
                var block = Expression.Block(call);
                MemberAssignment bind = Expression.Bind(pInfo, block);
                binds.Add(bind);
            }
            MemberInitExpression init = Expression.MemberInit(Expression.New(typeof(T)), binds.ToArray());
            return Expression.Lambda<Func<DataRow, T>>(init, paramter).Compile();
        }
    }
}
