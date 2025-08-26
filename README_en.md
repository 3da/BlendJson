# BlendJson
[Описание на русском языке](Readme.md)

This library allows describe flexible JSON configurations.
It allows to move some sections of JSON to external files.
It will produce single JSON after loading which can be mapped to specific C# class.

This library is based on Newtonsoft Json.NET.
Configuration format is fully compatible with normal JSON.

## Example 1.
This example demonstrates JSON settings, separated to multiple files and the result of loading.
It achieved by using special directives, whose names starts with symbol @. 

### Content Settings.json
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

### Content Colors.json
```json
[ "Red", "Green", "Blue" ]
```

### Content Websites.json
```json
[
  "microsoft.com",
  "apple.com"
]
```

### Content RemoteCredentials.json
```json
{
  "UserName": "adam",
  "Password": "sandler"
}
```

### Result of merging Settings.json
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
https://github.com/3da/BlendJson/tree/master/BlendJson.Tests



