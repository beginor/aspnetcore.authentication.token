# Custom Token Authentication for AspNetCore

Custom token authentication for Asp.NetCore 5.0+, allow access your webapi with `$token` in
query string param or the `Authorization` header with `$token {data_access_token}` format.

How to setup?

Please refer to [Setup.cs](test/WebTest/Startup.cs) .

Usage:

1. Simply put a `$token` parameter in query string, this is the prefered;

   ```url
   http://localhost:5000/weatherforcast?$token=your_data_access_token
   ```

1. Or put `$token {data_access_token}` in the `Authorization` header;

   ```json
   {
     "Authorization": "$token {data_access_token}"
   }
   ```
