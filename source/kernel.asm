section .text
global _start

extern program_a_main
extern program_b_main
extern program_с_main
extern program_с_a_main

_start:
	call program_с_a_main
	jmp halt

halt:
	hlt
	jmp halt