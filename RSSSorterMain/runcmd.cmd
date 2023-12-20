set retention=%1

if "%retention%"=="" (set retention=30) else (set retention=%1)

.\RSSSorter.exe .\InputLists .\HighvalItems.txt .\Discardlist.txt .\output %retention%