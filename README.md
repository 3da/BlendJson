# BlendJson
[English description](README_en.md)

![NuGet Version](https://img.shields.io/nuget/v/BlendJson)

Эта библиотека позволяет более гибко описывать конфигурацию приложения в формате JSON.
Бибилиотека позволяет вынести некоторые секции JSON в отдельные файлы.
В процессе загрузки при этом сформируется единый JSON, который можно смаппить на соответствующий класс в C#.

Эта библотека основана на Newtonsoft Json.NET.
Формат конфигурационных файлов полностью совместим с обычным форматом JSON.

## Пример 1.
Данный пример демонстрирует JSON настройки, разделённые на несколько файлов и результат их загрузки.
Это достигается путём использования соответствующих директив, названия которых начинаются символом @.

### Содержимое Settings.json
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

### Содержимое Colors.json
```json
[ "Red", "Green", "Blue" ]
```

### Содержимое Websites.json
```json
[
  "microsoft.com",
  "apple.com"
]
```

### Содержимое RemoteCredentials.json
```json
{
  "UserName": "adam",
  "Password": "sandler"
}
```

### Результат загрузки Settings.json
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

Вы можете найти больше примеров в модульных тестах:
https://github.com/3da/BlendJson/tree/master/BlendJson.Tests



