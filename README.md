### XML To DataTable
XmlNode의 데이터를 DataTable로 변환해주는 로직이다.
XML의 구조는 DataTable과 완벽하게 호환되지 않기 때문에 
기준이되는 노드를 설정하는 xpath 파라미터와
얼마나 깊게 탐색하여 Table을 구성할지 결정하는 depth 파라미터를 추가하였다.

```
/// <summary>
/// Xml 노드를 테이블로 전환합니다.
/// </summary>
/// <param name="self">노드</param>
/// <param name="xpath">타겟 XPath</param>
/// <param name="depth">몇 번째 depth까지 탐색하여 테이블을 생성할지 표시합니다.</param>
/// <returns></returns>
public static DataTable ToDataTable(this XmlNode self, string xpath, int depth = 1)
```

노드를 깊게 탐색하여 컬럼명이 길어질 경우 사용합니다.

```
/// <summary>
/// 컬럼명을 "-" 기준으로 줄여줍니다.
/// </summary>
/// <param name="self"></param>
/// <param name="depth"></param>
/// <param name="separator"></param>
public static void ShortenColumnName(this DataTable self, int depth = 1, string separator = "-")
```

### DataTable, DataRow To JObject, JArray
DataTable의 구조는 JSON 형태로 완벽하게 표현 가능하기 때문에
편의성 위주의 기능을 추가하여 구성했습니다.

```
/// <summary>
// DataRow를 JObject로 변환
/// </summary>
/// <param name="dataRow"></param>
/// <param name="validator">변환 로직</param>
/// <returns></returns>
public static JObject ToJObject(this DataRow dataRow)
public static JObject ToJObject(this DataRow dataRow, JsonDataValidator validator)
```

```
/// <summary>
// DataRow를 JObject로 변환
/// </summary>
/// <param name="dataRow"></param>
/// <returns></returns>
public static JArray ToJArray(this DataTable dataTable)
public static JArray ToJArray(this DataTable dataTable, JsonDataValidator validator)
public static JArray ToJArray(this DataRow[] dataRows)
public static JArray ToJArray(this DataRow[] dataRows, JsonDataValidator validator)
```

추가적인 설정 없이 DataTable나 DataRow을 즉시 변환할 수 있으며,
JsonDataValidator라는 검사기를 거쳐
필요 없는 컬럼을 제외하거나, 컬럼을 가공하여 JObject로 변환시킬 수 있습니다.

```
Validator = (obj, column, data, row) =>
{
    bool isMember = true;
    switch(column)
    {
        case "제외컬럼":
            isMember = false;
            break;
        case "가공컬럼":
            data = data + "1";
            break;
        case "가공컬럼2":
            data = data + "2";
            obj.Add("가공컬럼2-2", row["참조컬럼"].ToString() + data + "3");
            break;
                    
    }
    return new Tuple<bool, string, JToken>(isMember, column, JToken.FromObject(data));
};
```

위 예제처럼 제외할 컬럼을 설정하거나 컬럼의 데이터를 가공하여 제공하거나
해당 Row의 다른 데이터를 참조하여 새로운 컬럼을 추가할 수도 있습니다.

