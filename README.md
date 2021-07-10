# PVScan
PVScan is a mobile, desktop, and web application which allows users to scan barcodes of different formats and optionally save them to their device for logging purposes.

This application also allows users to create an optional profile via its backend server and sync barcodes across multiple devices via its API.

# Goals
* Easy to use
* Free & Open-Source
* Works on Mobile, Web, Desktop
* Works offline (except Web)

## Front-end

### Mobile Client
Mobile clients (iOS & Anroid) are implemented via Xamarin.Forms (C#)

### Web Client
Web client is implemented via React (TypeScript)

### Desktop Client
Desktop client is implemented via WPF (C#)

## Back-end
Spports an optional creation of user profile to sync barcodes across devices. Implemented via ASP.NET (C#)

### Authorization server
IdentityServer 4 is used on authorization server to provide OAuth + OpenIDConnect support. This is where you register, login, logout.

### API
Main API which provides endpoints to manipulate Barcode and User information data. This has SignalR hubs to notify other devices about changes to User information or Barcode information related to a user.

## Project structure
There are three main folders:
* `Source` - contains all the source code for the applications. Mobile and Desktop clients share the same logic via the Core project.
* `Tests` - contains all the XUnit test code for the applications.
* `Vendor` - contains all the source code for external dependencies used by PVScan. I use git sub-modules.

Both `Source` and `Tests` folders have subfolders which are named after the platform they are related to. Each subfolder in turn has a Visual Studio solution file which has all the neccesary projects loaded into it. 

`Source/Mobile/Mobile.sln` solution file is meant for Xamarin.Forms development for iOS and Android; open this solution file in order to start working on mobile client.

`Source/Web/Web.sln` soltuion file is meant for back-end development with ASP.NET; open this soltion file in order to start working on back-end.

`Source/Desktop/Windows/Desktop.Windows.sln` soltion file is meant for WPF development for Windows; open this soltion file in order to start working on Windows Desktop client.

## Configuring your enviroment

### Desktop Client
WPF client uses secrets. For now there are the following secrects:
 * `MapBoxKey`. This is an API key for MapBox. While this application is in development you'll have to get your own MapBox API key and use it by setting a dotnet secret with key `MapBoxKey` in the directory of WPF project. Type `dotnet user-secrets set "MapBoxKey" "YOUR_KEY"` to do that.
 * `SQLiteEncryptedKey_Debug`. This is a key which is used in debug mode to encrypt the local SQLite database which has sensetive user information.
 * `SQLiteEncryptedKey`. This is a key to which only I have access which is used to encrypt the local SQLite database which has sensetive user information. This is used in desktop releases.

### Web Client
N/A

### Mobile Client
There are no special configurations that have to be done in the Xamarin.Forms client aside from having an Apple Developer account to build and run this app on your iOS device.

### Back-end
There are two parts to the back-end, as I've described above. Both of them share the same MySQL database. So, you'll need MySQL Server installed on your machine in order to run the back-end.

## Running tests
Web, Dekstop.Windows, and Mobile soltion files are preconfigured to include the appropriate test projects and tests can be run directly from them for the appropriate platform. 
