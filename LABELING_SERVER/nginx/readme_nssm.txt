서비스를 구동하기 전에 먼저 설치해야한다.

참조: https://nssm.cc/usage

서비스 설치 및 구동
nssm.exe install <서비스명>   을 입력하면 UI 가 뜬다.
UI 화면에서 설치를 한 후

다음명령으로 서비스 실행한다.
sc query nginx
sc start nginx
tasklist /FI "IMAGENAME eq nginx*"


서비스 정지 및 삭제
sc stop nginx
nssm.exe remove nginx
