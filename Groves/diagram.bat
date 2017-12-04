@echo off

rem this batch file assumes that asciidoctor is installed
rem and asciidoctor-diagram is installed (http://asciidoctor.org/docs/asciidoctor-diagram/)
rem and that asciidoctor is in the path
rem and the diagrams you want to generated are in the current folder

for %%f in (.\*.adoc) do (
	echo %%f
	asciidoctor -r asciidoctor-diagram %%f
)
