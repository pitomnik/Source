// Header file for use with mtb.dll

typedef bool	(*Type_InitMotoBee)();								
typedef bool	(*Type_SetMotors)(int on1, int speed1, int on2, int speed2, int on3, int speed3, int on4, int speed4, int servo);
typedef void	(*Type_Digital_IO)(int *inputs, int outputs);
