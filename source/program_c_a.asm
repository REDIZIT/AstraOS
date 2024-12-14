section .text
    global program_с_a_main

program_с_a_main:
	
	mov rdi, "G"
	push rdi

	mov rdi, 0xB8000
	add rdi, 2
	push rdi	

	jmp write_vga_char

    ret

write_vga_char:

	pop rdi
	mov rbx, rdi

	pop rdi
	mov rcx, rdi

    mov byte [rbx], cl
    mov byte [rbx+1], 0x0F
    jmp halt

halt:
	hlt
	jmp halt