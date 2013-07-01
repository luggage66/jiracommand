jiracommand
===========

A simple utility to call a Jira 4.0 (old) webservice from the commandline.

Usage
-----

Run jiracommand without arguments to see options.

Samples
-------

* jiracommand config setserver "https://myjiraserver"
* jiracommand login -user bob
* jiracommand addissue -project PRJ -summary "A new issue" -description "The PC is on fire"
* jiracommand logout
