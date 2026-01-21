set retention=%1
set activitylog=%2
if "%retention%"=="" (set retention=30) else (set retention=%1)

if "%activitylog%"=="" (.\RSSSorter.exe .\RssLists .\Highval.txt .\Discard.txt .\output %retention% ) else (.\RSSSorter.exe .\RssLists .\Highval.txt .\Discard.txt .\output %retention% "%activitylog%" )