import struct, zlib
p="/workspace/WaterPipe-Android/app/src/main/res/drawable-nodpi/allwater.png"
data=open(p,"rb").read(); pos=8; idat=b""; w=h=None
while pos<len(data):
    ln=struct.unpack(">I",data[pos:pos+4])[0]; typ=data[pos+4:pos+8]; chunk=data[pos+8:pos+8+ln]
    if typ==b"IHDR": w,h,bitdepth,ct,cf,fl,inter=struct.unpack(">IIBBBBB",chunk)
    elif typ==b"IDAT": idat+=chunk
    elif typ==b"IEND": break
    pos+=12+ln
raw=zlib.decompress(idat); bpp=4; stride=w*bpp
def paeth(a,b,c):
    pa,pb,pc=b-a,c-b,abs(a+b-2*c)
    if pa<pb:return a if pa<pc else c
    return b if pb<pc else c
out=bytearray(len(raw)); prev=bytearray(stride); rl=stride+1
for y in range(h):
    row=raw[y*rl:(y+1)*rl]; ft=row[0]; line=bytearray(row[1:])
    for x in range(stride):
        a=line[x-bpp] if x>=bpp else 0; b=prev[x]; c=prev[x-bpp] if x>=bpp else 0
        if ft==1:line[x]=(line[x]+a)&0xFF
        elif ft==2:line[x]=(line[x]+b)&0xFF
        elif ft==3:line[x]=(line[x]+((a+b)>>1))&0xFF
        elif ft==4:line[x]=(line[x]+paeth(a,b,c))&0xFF
    out[y*stride:(y+1)*stride]=line; prev=line
def a(x,y):return out[y*stride+x*bpp+3]
print("atlas size:",w,"x",h)
# 4x downsample full width, print every row (75px => ~19 rows per tile row)
for y in range(0,h,3):
    s=""
    for x in range(0,w,4):
        al=a(x,y)
        # max alpha in 4x3 block
        m=a(x,y)
        for dx in range(4):
            for dy in range(3):
                v=a(min(x+dx,w-1),min(y+dy,h-1))
                if v>m:m=v
        s+='#' if m>=200 else('+' if m>=100 else('.' if m>=40 else('-' if m>=10 else(' ' if m>=3 else ','))))
    print("%3d %s"%(y,s))