/ Test word needs to be set to nonzero value before
/ starting this program

begin,	cla		/ Load step from test word
	lat		/ Load step from test word
	dac  _step	/ store step
	cla		/ clear accumulator
loop,	add  step	/ add step to accumulator
	szo		/ keep going if no overflow
	hlt		/ otherwise stop
	jmp  loop	/ keep looping

	variables
	start begin






