aVoice Push
==========

aVoice Push is a simple app for Windows Phone 8.1 that hosts Google Voice's mobile web page (https://www.google.com/voice/m),
as well as providing push notifications in the form of [Tiles, Badges, and Toasts](http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh779725.aspx).
By using [Google OAuth 2.0](https://developers.google.com/accounts/docs/OAuth2) to authenticate users, aVoice Push is the only 3rd party
Google Voice app that *doesn't have access to the users's password*.

Project Structure
-----------------

There are 3 main components to getting this working:

Name | Directory | Description
---- | --------- | -----------
Azure Cloud Service | `aVoicePushAzure/` | Holds a Worker Role that parses Google Voice SMTP emails ([more info](http://cryclops.com/apps/avoice/#push))
Universal Windows App | `aVoicePushClient/` | A XAML/C# app for Windows Phone 8.1 (Windows 8.1 not implemented yet)
Azure Mobile Service | *not included* | Maintains DB of active users, handles authentication and push notifications

`aVoicePushAzure` has a parser that is broken into components and fully unit tested. It's the most complex piece. `aVoicePushClient`'s job
is to authenticate, register for push notifications, then display the web page.

Contributing
------------

Improvements are welcome! I hold the keys for pushing out updates to the production service & app and will create
new versions as appropriate. You are free to deploy your own sister services and apps as well. All I ask is that
the main constraint of this app is respected - *don't collect the user's password*.
