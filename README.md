#Quick intro

This Visual Studio 2013 solution is a Proof-of-Concept showing how to successfully manage to read a number of WAV audio files from disk and concatenate them *in memory*, finally writing the result into an output WMA audio file.

The solution is a WPF application showing just a panel with buttons, each one launching one of the tentative implementations tried to get the right result. Without much ado, we tell you right away that what has worked for us is on the last row, that is using NAudio *MediaFoundation* API with an implicit or explicit WMA converter. You can see those highlighted in the following screenshot: 

![mainwindow-screenshot](https://cloud.githubusercontent.com/assets/3185573/6617213/b89781e6-c8b6-11e4-9e7c-2c96f1294db5.png)

HTH
