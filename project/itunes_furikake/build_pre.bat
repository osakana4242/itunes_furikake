@rem dict�t�H���_�� Debug,Relese�t�H���_���ɃR�s�[����.
@echo off
%~d0
cd %~p0

SET RES_DIR=outside_res
SET COPY_DIR_NAME=dict

SET TARGET_DIR=bin\Debug
rmdir "%TARGET_DIR%\%COPY_DIR_NAME%" /s /q
xcopy "%RES_DIR%\%COPY_DIR_NAME%" "%TARGET_DIR%\%COPY_DIR_NAME%\"

SET TARGET_DIR=bin\Release
rmdir "%TARGET_DIR%\%COPY_DIR_NAME%" /s /q
xcopy "%RES_DIR%\%COPY_DIR_NAME%" "%TARGET_DIR%\%COPY_DIR_NAME%\"
