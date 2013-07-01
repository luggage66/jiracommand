jiracommand
===========

A simple utility to call a Jira 4.0 (old) webservice from the commandline.

Download
--------

Pre-built version: https://s3.amazonaws.com/jiracommand-releases/jiracommand-v0.1.zip

Usage
-----

Run jiracommand without arguments to see options.

Example
-------

    jiracommand config setserver "https://myjiraserver"
    jiracommand login -user bob
    jiracommand addissue -project PRJ -summary "A new issue" -description "The PC is on fire"
    jiracommand logout
