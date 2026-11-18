# 发布 NuGet 包

使用 nuget.org 的 **Trusted Publishing（OIDC）** 发布，仓库中不再保存长期 API key。

## 1. 发布元数据：release.env

`release.env` 是发布元数据的唯一来源，共四个变量：

```bash
export PACKAGE_VERSION="10.0.1"          # 必须大于 nuget.org 上已有版本
export PACKAGE_RELEASE_NOTES="Update to .NET 10.0.1;"
export PACKAGE_TAGS="authentication,aspnetcore"
export PACKAGE_PROJECT_URL="https://github.com/beginor/aspnetcore.authentication.token"
```

版本号规则：大版本与 .NET 运行时一致（如 .NET 10 → `10.x`），每次发布只递增一个小版本。

## 2. props 引用方式

`src/package-version.props` 把环境变量映射为包属性：

```xml
<PropertyGroup>
  <PackageVersion>$(PACKAGE_VERSION)</PackageVersion>
  <PackageReleaseNotes>$(PACKAGE_RELEASE_NOTES)</PackageReleaseNotes>
  <PackageTags>$(PACKAGE_TAGS)</PackageTags>
  <PackageProjectUrl>$(PACKAGE_PROJECT_URL)</PackageProjectUrl>
</PropertyGroup>
```

所有 csproj 通过 `<Import Project="..\package-version.props" />` 引用它。
MSBuild 会把同名环境变量当作属性读取，因此**打包前必须先 `source release.env`**，否则版本会退化成 `1.0.0`。

## 3. nuget.org Trusted Publishing 策略

nuget.org → 用户菜单 → **Trusted Publishing** → **Create**，字段如下：

| 字段 | 值 |
| --- | --- |
| Package owner | `beginor` |
| Repository Owner | `beginor` |
| Repository | `aspnetcore.authentication.token` |
| Workflow File | `publish-nuget.yml`（**只填文件名**，不要带 `.github/workflows/` 前缀） |
| Environment | 留空 |
| Scopes | Push new packages and package versions |
| Glob | `Beginor.AspNetCore.Authentication.Token*`（包 ID 前缀 + `*`） |

> 注意：`Foo.*` 只匹配带后缀的包，不匹配 `Foo` 本身，会导致 push 返回 403。

## 4. GitHub 仓库变量

GitHub 仓库 → Settings → Secrets and variables → Actions → **Variables** → New repository variable：

- Name: `NUGET_USER`
- Value: nuget.org 的**用户名**（不是邮箱）

工作流通过 `${{ vars.NUGET_USER }}` 传给 `NuGet/login@v1`。

## 5. 工作流要点

`.github/workflows/publish-nuget.yml`：tag `[0-9]*` 触发 + `workflow_dispatch`，权限只有 `contents: read` 和 `id-token: write`。

- `NuGet/login@v1` 用 OIDC token 换取**临时 API key，有效期仅 1 小时**，所以 push 必须紧跟 login；
- OIDC token 本身**不能直接当 api-key** 使用，必须用 login 步骤输出的 `NUGET_API_KEY`；
- push 加 `--skip-duplicate`，重复发布同一版本时工作流不会失败。

## 6. 发布步骤

```bash
# 1. 修改 release.env 中的版本与发布说明
# 2. 本地验证 nuspec
source ./release.env && dotnet pack src/*/*.csproj -c Release --output ./nupkgs/
unzip -p ./nupkgs/*.nupkg '*.nuspec' | grep -E 'version|releaseNotes|tags|projectUrl'
rm -rf ./nupkgs

# 3. 提交
git add -A && git commit -m "release: v${PACKAGE_VERSION}"

# 4. 打标签（标签名必须与 PACKAGE_VERSION 完全一致）
git tag "${PACKAGE_VERSION}"

# 5. 推送分支和标签
git push origin main
git push origin "${PACKAGE_VERSION}"
```

推送标签后 Actions 会执行 publish-nuget，成功后 nuget.org 上出现新版本。
状态显示 `Validating` 属正常，索引最长约 1 小时。

## 7. 常见问题

- **`No matching trust policy` / 403 `API key is invalid`**：策略字段与仓库/工作流不匹配。检查 Repository、Workflow File（只填文件名）、Glob 前缀、Environment 是否留空；Glob 写成 `Foo.*` 会漏掉 `Foo` 本身，应写 `Foo*`。
- **403 / key 过期**：临时 key 只有 1 小时，且不能跨步骤复用；确保 push 紧跟在 login 之后。
- **版本退化成 1.0.0**：忘记 `source release.env`，或 csproj 没有 import `package-version.props`。
- **策略显示 Pending**：首次使用需先成功发布一次（或等待 nuget.org 校验），确认字段无误后重试。
- **工作流成功但版本没更新**：被 `--skip-duplicate` 跳过，说明该版本已存在，需要递增 `PACKAGE_VERSION`。
