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
extern program_ram_main
extern program_disks_main
extern program_proper_calls_main
extern program__main

_start:
	call program__main
	jmp halt

halt:
	hlt
	jmp halt