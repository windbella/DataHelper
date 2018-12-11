using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataHelper
{
    /// <summary>
    /// Xml 유틸 클래스
    /// </summary>
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// Xml 노드를 테이블로 전환합니다.
        /// AddData 함수를 이용합니다.
        /// </summary>
        /// <param name="self">노드</param>
        /// <param name="xpath">타겟 XPath</param>
        /// <param name="depth">몇 번째 depth까지 탐색하여 테이블을 생성할지 표시합니다.</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this XmlNode self, string xpath, int depth = 1)
        {
            DataTable table = new DataTable();
            foreach (XmlNode item in self.SelectNodes(xpath))
            {
                DataRow row = table.NewRow();
                row.AddData(item, string.Empty, 1, depth);
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// 컬럼명을 "-" 기준으로 줄여줍니다.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="depth"></param>
        /// <param name="separator"></param>
        public static void ShortenColumnName(this DataTable self, int depth = 1, string separator = "-")
        {
            foreach (DataColumn item in self.Columns)
            {
                for (int i = 0; i < depth; i++)
                {
                    int first = item.ColumnName.IndexOf(separator);
                    if (first >= 0)
                    {
                        item.ColumnName = item.ColumnName.Substring(first + 1, item.ColumnName.Length - first - 1);
                    }
                }
            }
        }

        /// <summary>
        /// DataRow에 Node의 데이터를 추가합니다.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="depth"></param>
        public static void AddData(this DataRow self, XmlNode node, string parent, int index, int depth)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    string name = node.Name;
                    if (index > 1)
                    {
                        name = string.Format("{0}-{1}", name, index);
                    }
                    string columnName = string.Empty.Equals(parent) ? string.Format("{1}-{2}", name, attribute.Name) : string.Format("{0}-{1}-{2}", parent, name, attribute.Name);
                    if (!self.Table.Columns.Contains(columnName))
                    {
                        self.Table.Columns.Add(columnName);
                    }
                    self[columnName] = attribute.Value.ToDBValue();
                }
            }
            if (node.ChildNodes.Count > 0)
            {
                if (depth > 0)
                {
                    parent = string.Empty.Equals(parent) ? node.Name : string.Format("{0}-{1}", parent, node.Name);
                    List<string> childList = new List<string>();
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        string childName = child.Name;
                        childList.Add(child.Name);
                        index = childList.Count(c => c == child.Name);
                        self.AddData(child, parent, index, depth - 1);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(node.InnerText))
            {
                string name = node.Name;
                string columnName = string.Empty.Equals(parent) ? name : string.Format("{0}-{1}", parent, name);
                if (!self.Table.Columns.Contains(columnName))
                {
                    self.Table.Columns.Add(columnName);
                }
                self[columnName] = node.InnerText.ToDBValue();
            }
        }

        /// <summary>
        /// object를 DB데이터 형태로 변환합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ToDBValue(this object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            else if (value is string && string.IsNullOrEmpty((string)value))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
    }
}
