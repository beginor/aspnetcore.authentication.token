# CLAUDE.md

## 项目说明

`Beginor.AspNetCore.Authentication.Token`：为 ASP.NET Core 提供基于 `$token` 的自定义认证。

- `src/AspNetCore.Authentication.Token`：包源码
- `test/WebTest`：示例/测试站点
- `release.env`：发布元数据唯一来源

## 构建与测试

```bash
dotnet build AspNetCore.Authentication.Token.slnx
dotnet test AspNetCore.Authentication.Token.slnx
```

## 发布

使用 nuget.org Trusted Publishing（OIDC），无需长期 API key：

1. 修改 `release.env` 的 `PACKAGE_VERSION`（大版本与 .NET 一致，递增一个小版本）与发布说明；
2. 本地 `source ./release.env && dotnet pack src/*/*.csproj -c Release --output ./nupkgs/` 验证 nuspec 后提交；
3. `git tag <版本号>` 并 `git push origin main <版本号>`，由 `publish-nuget.yml` 自动发布。

详见 [docs/publish-nuget.md](docs/publish-nuget.md)。
