@rem �J�����g�f�B���N�g�����o�b�`�t�@�C���̒u���ꏊ�ɕύX.
%~d0
cd %~p0

SET TARGET_DIR=bin\Debug
SET RES_DIR=outside_res
SET COPY_DIR_NAME=dict

rmdir "%TARGET_DIR%\%COPY_DIR_NAME%" /s /q
xcopy "%RES_DIR%\%COPY_DIR_NAME%" "%TARGET_DIR%\%COPY_DIR_NAME%\"
