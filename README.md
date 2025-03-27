# MAUI HyperTrack SDK

[![GitHub](https://img.shields.io/github/license/hypertrack/sdk-maui?color=orange)](./LICENSE)
[![NuGet](https://img.shields.io/nuget/v/HyperTrack.SDK.MAUI.svg)](https://www.nuget.org/packages/HyperTrack.SDK.MAUI)
[![iOS SDK](https://img.shields.io/badge/iOS%20SDK-5.11.3-brightgreen.svg)](https://github.com/hypertrack/sdk-ios)
[![Android SDK](https://img.shields.io/badge/Android%20SDK-7.11.4-brightgreen.svg)](https://github.com/hypertrack/sdk-android)

[HyperTrack](https://www.hypertrack.com) lets you add live location tracking to your mobile app. Live location is made available along with ongoing activity, tracking controls and tracking outage with reasons.

MAUI HyperTrack SDK is a wrapper around native iOS and Android SDKs that allows to integrate HyperTrack into .NET MAUI apps.

For information about how to get started with MAUI HyperTrack SDK, please check this [Guide](https://www.hypertrack.com/docs/install-sdk-maui).

## Installation

Add following to your project .csproj file:

```
<ItemGroup>
		<PackageReference Include="HyperTrack.SDK.MAUI" Version="VERSION" />

		<!-- Workaround for MAUI bug that causes Android libraries that povide .jar to not be packed into the package -->
		<AndroidMavenLibrary Include="com.hypertrack:sdk-android-model" Version="7.11.4" Repository="https://s3-us-west-2.amazonaws.com/m2.hypertrack.com/" Bind="false" Condition="'$(TargetFramework)' == 'net9.0-android'"/>
		<AndroidMavenLibrary Include="org.jetbrains.kotlinx:kotlinx-serialization-json-jvm" Version="1.3.3" Bind="false" Condition="'$(TargetFramework)' == 'net9.0-android'"/>
		<AndroidIgnoredJavaDependency Include="org.jetbrains.kotlin:kotlin-stdlib:1.6.21" />
		<AndroidIgnoredJavaDependency Include="org.jetbrains.kotlin:kotlin-stdlib-jdk8:1.6.21" />
		<AndroidIgnoredJavaDependency Include="org.jetbrains.kotlin:kotlin-stdlib-common:1.6.21" />
		<AndroidIgnoredJavaDependency Include="org.jetbrains.kotlinx:kotlinx-serialization-core-jvm:1.3.3" />
</ItemGroup>
```

## Sample code

[Quickstart MAUI app](https://github.com/hypertrack/quickstart-maui)

## Requirements

[Requirements for MAUI HyperTrack SDK](https://hypertrack.com/docs/install-sdk-maui#requirements)

## Contributing

If you want to contribute check [CONTRIBUTING.md](CONTRIBUTING.md)
