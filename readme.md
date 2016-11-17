Implementations of the ISerialization and IBodyDeserializer interfaces, based on [ServiceStack.Text](https://github.com/ServiceStack/ServiceStack.Text), for [Nancy](http://nancyfx.org)

## Usage

Start of by installing the `Nancy.Serialization.ServiceStack` nuget

When Nancy detects that the `ServiceStackJsonSerializer` and `ServiceStackBodyDeserializer` types are available in the AppDomain, of your application, it will assume you want to use them, rather than the default ones.

### Customization

If you want to customize the behavior of ServiceStack.Text, you use the normal `JsConfig` that is provided by the serializer.

For instance, if you wanted to emit camel-cased names in your json, you would set the following property

```c#
JsConfig.EmitCamelCaseNames = true;
```

Consult the ServiceStack.Text documentation, for information on the configurations you are able to perform.

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).

## Contribution License Agreement

Contributing to Nancy requires you to sign a [contribution license agreement](https://cla2.dotnetfoundation.org/) (CLA) for anything other than a trivial change. By signing the contribution license agreement, the community is free to use your contribution to .NET Foundation projects.

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).

## Copyright

Copyright © 2010 Andreas Håkansson, Steven Robbins and contributors

## License

Nancy.Serialization.ServiceStack is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
