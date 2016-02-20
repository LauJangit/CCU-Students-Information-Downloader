#include<stdio.h>
#include<string.h>
#include<stdlib.h>
#include<time.h>
//ȫ�ֱ���
int Status=0;
//��Ϣ���� 
struct inf
{
	char CollegeName[50];
	char Major[50];
	char CollegeType[20];
	char Class[30];
	char SchoolID[20];
	char StudentName[20];
	char Sex[5];
	char StudentBirth[20];
	char PhoneNum[20];
	char HighschoolName[50];
	char HomeAddress[1000];
	char PersonalID[20];
}Inf[1];
//д�뺯��
int WirteData()
{
	if(Status==1)
	{
		Status=0;
		return -1;
	}
	FILE *CSV=fopen("Done\\INFMATION.csv","a");
	fprintf(CSV,"%s,%s,%s,%s,%s,",Inf[0].StudentName,Inf[0].Sex,Inf[0].Class,Inf[0].SchoolID,Inf[0].CollegeType);
	fprintf(CSV,"%s,%s,%s,%s,%s,",Inf[0].Major,Inf[0].CollegeName,Inf[0].StudentBirth,Inf[0].PhoneNum,Inf[0].PersonalID);
	fprintf(CSV,"%s,%s\n",Inf[0].HighschoolName,Inf[0].HomeAddress);
	fclose(CSV);
	return 0;
}
//ɸѡ���� 
int Filter(char *filename)
{
	memset(&Inf[0].Class,0,1305);
	char Content[2000]={""},TEMP[100]={""};
	FILE *Input=fopen(filename,"rw+");
	if(!Input)
	{
		printf("%d",Input);
		Status=1;
		return -1;
	}
	while(fgets(Content,2000,Input))
	{
		printf("%s",Content);
		if(strstr(Content,"ѧ��������Ϣ")!=NULL) 
		{
			printf("%s",Content);
			system("pause"); 
			break;
		}
		if(Content==NULL)
		{
			Status=1;
			return -2;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"</head>"))
		{
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================Ժϵ
	{
		if(strstr(Content,"Ժϵ")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='>')
			{
				Count_Ct++;
			}
			Count_Ct++;
			while(Content[Count_Ct]!='<')
			{
				Inf[0].CollegeName[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================רҵ
	{
		if(strstr(Content,"רҵ")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='>')
			{
				Count_Ct++;
			}
			Count_Ct++;
			while(Content[Count_Ct]!='<')
			{
				Inf[0].Major[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================ѧ��
	{
		if(strstr(Content,"ѧ��")!=NULL)
		{
			int Count_Ct=0,Count_CN=0; 
			char TEMPCT[30]={""};
			while(Content[Count_Ct]!='>')
			{
				Count_Ct++;
			}
			Count_Ct++;
			while(Content[Count_Ct]!='<')
			{
				TEMPCT[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			switch (atoi(TEMPCT)) 
			{
				case 3:strcpy(Inf[0].CollegeType,"ר��");break;
				case 4:strcpy(Inf[0].CollegeType,"����"); break;
				default :strcpy(Inf[0].CollegeType,"����"); break;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================�༶ 
	{
		if(strstr(Content,"�༶")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='>')
			{
				Count_Ct++;
			}
			Count_Ct++;
			while(Content[Count_Ct]!='<')
			{
				Inf[0].Class[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================ѧ��
	{
		if(strstr(Content,"ѧ��")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='>')
			{
				Count_Ct++;
			}
			Count_Ct++;
			while(Content[Count_Ct]!='<')
			{
				Inf[0].SchoolID[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================����
	{
		if(strstr(Content,"tbxsxm")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				Inf[0].StudentName[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================�Ա�
	{
		if(strstr(Content,"selected"))
		{
			if(strstr(Content,"1"))
			{
				strcpy(TEMP,"��");
			}
			if(strstr(Content,"2"))
			{
				strcpy(TEMP,"Ů");
			}
			strcpy(Inf[0].Sex,TEMP);
			memset(TEMP,0,sizeof(TEMP));
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================��������
	{
		if(strstr(Content,"��������"))
		{
			break;
		}
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"tbcsrq")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				TEMP[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			strcpy(Inf[0].StudentBirth,TEMP);
			memset(TEMP,0,sizeof(TEMP));
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================���˵绰
	{
		if(strstr(Content,"���˵绰"))
		{
			break;
		}
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"tbbrlxdh")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				Inf[0].PhoneNum[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================���� 
	{
		if(strstr(Content,"��ѧǰ������λ"))
		{
			break;
		}
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"tbrxqgzdw")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				Inf[0].HighschoolName[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================��ͥסַ 
	{
		if(strstr(Content,"��ͥ��סַ"))
		{
			break;
		}
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"tbjtxzdz")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				Inf[0].HomeAddress[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	while(fgets(Content,2000,Input))//<=========================================================================���֤��� 
	{
		if(strstr(Content,"���֤���"))
		{
			break;
		}
	}
	while(fgets(Content,2000,Input))
	{
		if(strstr(Content,"tbsfzh")!=NULL)
		{
			int Count_Ct=0,Count_CN=0;
			while(Content[Count_Ct]!='v')
			{
				Count_Ct++;
			}
			Count_Ct=Count_Ct+7;
			while(Content[Count_Ct]!='"')
			{
				Inf[0].PersonalID[Count_CN]=Content[Count_Ct];
				Count_Ct++;
				Count_CN++;
			}
			break;
		}
		memset(Content,0,2000);
	}
	fclose(Input);
	return 0; 
}
int main()
{
	system("rd Done /s /q");
	system("cls");
	char Content[300]={""},ClearEnter=0,Position[300]={""},OutputPosition[320]={""};;
	system("dir >FileList.db");
	getcwd(Position,300);
	strcat(Position,"\\Done");
	if(access(OutputPosition,6)==-1)
	{
		system("md Done\\");
	}
	int Count=0,RWD=5,RF=5;
	FILE *FailedList=fopen("Done\\Failes.txt","a"),*List=fopen("FileList.db","rw+");
	while(fgets(Content,300,List))
	{
		if(strstr(Content,"txt"))
		{
			char *Position=strstr(Content,"txt");
			while(1)
			{
				Position--;
				if(*(Position-1)==' ')
				{
					break;
				}
			}
			for(ClearEnter=0;;ClearEnter++)
			{
				if(Position[ClearEnter]=='\n')
				{
					Position[ClearEnter]='\0';
					break;
				}
			}
			system("cls");
			RF=Filter(Position);
			RWD=WirteData();
			printf("���ڴ����%d���ļ� Filter Status:%d File:|%s|",Count+1,RWD,Position);
			if(RWD!=0||RF!=0)
			{
				fprintf(FailedList,"�����ļ���%s|ɸѡ�������أ�%d|д�뺯�����أ�%d\n",Position,RF,RWD);
			}
			RWD=RF=5;
			Count++;
		}
	}
	fclose(List);
	fclose(FailedList);
	system("del FileList.db");
}
