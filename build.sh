nasm -f elf64 -o obj/boot.o source/boot.asm
nasm -f elf64 -o obj/kernel.o source/kernel.asm
nasm -f elf64 -o obj/program_a.o source/program_a.asm
nasm -f elf64 -o obj/program_b.o source/program_b.asm
nasm -f elf64 -o obj/program_с.o source/program_с.asm
nasm -f elf64 -o obj/program_с_a.o source/program_c_a.asm

ld -n -o iso/boot/kernel.bin obj/boot.o obj/kernel.o obj/program_a.o obj/program_b.o obj/program_с.o obj/program_с_a.o

grub-mkrescue -o myos.iso iso/