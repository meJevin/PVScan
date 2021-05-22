# PVScan
PVScan is a mobile, desktop, and web application which allows users to scan barcodes of different formats and optionally save them to their device for logging purposes.

This application also allows users to create an optional profile via its backend server and sync barcodes across multiple devices.

# Goals
* Easy to use
* Free & Open-Source
* Works on Mobile, Web, Desktop
* Works offline (except Web)

# Front-end

## Mobile
Mobile clients (iOS & Anroid) are implemented via Xamarin.Forms (C#)

## Web
Web client is implemented via React (TypeScript)

## Desktop
Desktop client is implemented via WPF (C#)

# Back-end
Backend which supports an optional creation of user profile to sync barcodes across devices is implemented via ASP.NET (C#)

## Authorization server
IdentityServer 4 is used on authorization server to provide OAuth + OpenIDConnect support.

## API
Main API is built using ASP.NET with MVC pattern in mind. I figured this project is not that big to implement CQRS
