int pwm_a = 3;
int pwm_b = 11;
int dir_a = 12;
int dir_b = 13;
int speed = 220;

void setup()
{
	pinMode(pwm_a, OUTPUT);
	pinMode(pwm_b, OUTPUT);
	pinMode(dir_a, OUTPUT);
	pinMode(dir_b, OUTPUT);

	analogWrite(pwm_a, speed);
	analogWrite(pwm_b, speed);
}

void loop()
{
	digitalWrite(dir_a, LOW);
	digitalWrite(dir_b, LOW);

	delay(1000);

	analogWrite(pwm_a, speed);
	analogWrite(pwm_b, speed);

	delay(3000);

	digitalWrite(dir_a, HIGH);
	digitalWrite(dir_b, HIGH);

	delay(1000);

	analogWrite(pwm_a, speed);
	analogWrite(pwm_b, speed);

	delay(3000);
}
