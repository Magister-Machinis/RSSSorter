set retention=%1

if "%retention%"=="" (set retention=30) else (set retention=%1)

.\RSSSorter.exe .\RssLists .\Highval.txt .\Discard.txt .\output %retention%