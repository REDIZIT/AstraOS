section .text
global _start

extern program_a_main
extern program_b_main
extern program_с_main
extern program_с_a_main
extern program_с_b_main
extern program_с_c_main
extern program_d_main
extern program_e_main
extern program_int_main

_start:
	call program_int_main
	jmp halt

halt:
	hlt
	jmp halt