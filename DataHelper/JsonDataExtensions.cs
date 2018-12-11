using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataHelper
{
    /// <summary>
    /// DataRow를 JObject로 변경 시키는 로직 설정
    /// </summary>
    /// <param name="obj">변경 될 JObject 객체, DataRow의 컬럼과 1:1 매칭되지 않는 경우 수동으로 넣어줄 때 사용한다</param>
    /// <param name="column">컬럼</param>
    /// <param name="data">데이터</param>
    /// <param name="row">데이터 변경 시 같은 DataRow의 다른 데이터를 사용할 경우 사용한다.</param>
    /// <returns>JObject 변경 로직으로 전달 될 결과, 컬럼 사용 유무, JObject에 사용 될 컬럼(Key), JObject에 사용 될 데이터(Value)</returns>
    public delegate Tuple<bool, string, JToken> JsonDataValidator(JObject obj, string column, object data, DataRow row);

    public static class JsonDataExtensions
    {
        static JsonDataExtensions()
        {
            DefaultValidator = (obj, column, data, row) =>
            {
                return new Tuple<bool, string, JToken>(true, column, data.ToString());
            };
        }

        /// <summary>
        /// 기본 변경 로직 설정
        /// </summary>
        public static JsonDataValidator DefaultValidator { get; set; }

        /// <summary>
        /// DataRow를 JObject로 변환
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="validator">변환 로직</param>
        /// <returns></returns>
        public static JObject ToJObject(this DataRow dataRow, JsonDataValidator validator)
        {
            JObject result = new JObject();
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                Tuple<bool, string, JToken> data = validator(result, column.ColumnName, dataRow[column.ColumnName], dataRow);
                if (data.Item1)
                {
                    result[data.Item2] = data.Item3;
                }
            }
            return result;
        }

        /// <summary>
        // DataRow를 JObject로 변환
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static JObject ToJObject(this DataRow dataRow)
        {
            return dataRow.ToJObject(DefaultValidator);
        }

        /// <summary>
        /// DataTable을 JArray로 변환
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="validator">변환 로직</param>
        /// <returns></returns>
        public static JArray ToJArray(this DataTable dataTable, JsonDataValidator validator)
        {
            JArray result = new JArray();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                result.Add(dataRow.ToJObject(validator));
            }
            return result;
        }

        /// <summary>
        /// DataTable을 JArray로 변환
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static JArray ToJArray(this DataTable dataTable)
        {
            return dataTable.ToJArray(DefaultValidator);
        }

        /// <summary>
        /// DataRow[]을 JArray로 변환
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="validator">변환 로직</param>
        /// <returns></returns>
        public static JArray ToJArray(this DataRow[] dataRows, JsonDataValidator validator)
        {
            JArray result = new JArray();
            foreach (DataRow dataRow in dataRows)
            {
                result.Add(dataRow.ToJObject(validator));
            }
            return result;
        }

        /// <summary>
        /// DataRow[]을 JArray로 변환
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static JArray ToJArray(this DataRow[] dataRows)
        {
            return dataRows.ToJArray(DefaultValidator);
        }
    }
}
