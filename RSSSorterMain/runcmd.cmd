set retention=%1

if "%retention%"=="" (set retention=30) else (set retention=%1)

.\RSSSorter.exe .\RssLists .\HighvalItems.txt .\Discardlist.txt .\output %retention%