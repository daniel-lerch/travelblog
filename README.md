# TravelBlog

TravelBlog is a minimalistic CMS to share your travel experience with friends.
The owner can confirm or reject registrations and limit access to confirmed users.

## Installing
The easiest way of deployment is to build your own Docker container:  
`docker build -t daniel-lerch/travelblog:latest https://github.com/daniel-lerch/travelblog.git`

You will want to mount several paths to persist data across container rebuilds:
- `/app/appsettings.json`
- `/app/travelblog.db`
- `/app/media`
- `/app/secrets`

## Usage
TravelBlog is designed for power users that know how to write Markdown and HTML.
Multiple user accounts are currently not supported and all settings have to be statically configured in `appsettings.json`.
