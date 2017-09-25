@echo off

rem this batch file assumes that pngcrush and jpegtran (and gifsicle)
rem are in the path
rem and the images you want to optimize are in the current folder

mkdir old

rem ******** PNG
move *.png .\old
pngcrush -brute -d . .\old\*.png

rem ******** JPG
move *.jpg .\old
for %%f in (.\old\*.jpg) do (
	echo %%f
	jpegtran %%f .\%%~nf.jpg
)

rem ******** GIF
rem *** not using this because it seems to give animated GIFs some hiccups
rem *** I don't use many non-animated GIFs
rem *** but I'll leave this here in case
rem copy *.gif .\old
rem gifsicle --batch -O3 -Okeep-empty *.gif