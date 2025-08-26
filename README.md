# BlendJson
This library allows describe flexible JSON configurations.
It allows to move some sections of JSON to external files.
It will produce single JSON after loading which can be mapped to specific C# class.

This library is based on Newtonsoft Json.NET.
Configuration format is fully compatible with normal JSON.

Эта библиотека позволяет более гибко описывать конфигурацию приложения в формате JSON.
Бибилиотека позволяет вынести некоторые секции JSON в отдельные файлы.
В процессе загрузки при этом сформируется единый JSON, который можно смаппить на соответствующий класс в C#.

Эта библотека основана на Newtonsoft Json.NET.
Формат конфигурационных файлов полностью совместим с обычным форматом JSON.

## Example (Пример) 1.
Данный пример демонстрирует JSON настройки, разделённые на несколько файлов и результат их загрузки.
Это достигается путём использования соответствующих директив, названия которых начинаются символом @.

This example demonstrates JSON settings, separated to multiple files and the result of loading.
It achieved by using special directives, whose names starts with symbol @. 

### Content (Содержимое) Settings.json
```json
{
  "NormalProperty": "HelloWorld",
  "Colors": {
    "@LoadFrom": "Colors"
  },
  "Websites": [
    "google.com",
    {
      "@MergeArrayWith": "Websites"
    }
  ],
  "RemoteService": {
    "Url": "http://localhost:9999/",
    "@MergeWith": "RemoteCredentials"
  }
}
```

### Content (Содержимое) Colors.json
```json
[ "Red", "Green", "Blue" ]
```

### Content (Содержимое) Websites.json
```json
[
  "microsoft.com",
  "apple.com"
]
```

### Content (Содержимое) RemoteCredentials.json
```json
{
  "UserName": "adam",
  "Password": "sandler"
}
```

### Result of loading (Результат загрузки) Settings.json
```json
{
  "NormalProperty": "HelloWorld",
  "Colors": [ "Red", "Green", "Blue" ],
  "Websites": [
    "google.com",
    "microsoft.com",
    "apple.com"
  ],
  "RemoteService": {
    "UserName": "adam",
    "Password": "sandler",
    "Url": "http://localhost:9999/"
  }
}
```


You can find more examples in unit tests:
Вы можете найти больше примеров в модульных тестах:
https://github.com/3da/BlendJson/tree/master/BlendJson.Tests



