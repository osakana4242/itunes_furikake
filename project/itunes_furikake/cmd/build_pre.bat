@rem dictフォルダを Debug,Releseフォルダ下にコピーする.
@echo off
%~d0
cd %~p0

SET PRJ_DIR=..\
SET RES_DIR=%PRJ_DIR%outside_res
SET COPY_DIR_NAME=dict

SET TARGET_DIR=%PRJ_DIR%bin\Debug
rmdir "%TARGET_DIR%\%COPY_DIR_NAME%" /s /q
xcopy "%RES_DIR%\%COPY_DIR_NAME%" "%TARGET_DIR%\%COPY_DIR_NAME%\"

SET TARGET_DIR=%PRJ_DIR%bin\Release
rmdir "%TARGET_DIR%\%COPY_DIR_NAME%" /s /q
xcopy "%RES_DIR%\%COPY_DIR_NAME%" "%TARGET_DIR%\%COPY_DIR_NAME%\"
