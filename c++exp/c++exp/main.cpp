#include <iostream>

using namespace std;
class mainClass
{
public:
	virtual int p(){
		return 5;
	}
	virtual double p2(){
		return 5;
	}
};
class inClass:public mainClass
{

private:
	virtual int p(){
		return 6;
	}
	int k;
};
int main()
{
	int a=0;
	mainClass h,*p=NULL;
	cout <<a++<<a;
	cout <<endl<<sizeof(h);
	cout<<endl<<h.p();
	p = new inClass();
	cout <<endl<<sizeof(inClass);
	cout<<endl<<p->p();
	return 0;
}