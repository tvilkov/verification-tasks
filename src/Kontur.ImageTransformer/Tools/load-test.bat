rem @echo off
rem ab -v 1 -s 1 -n 50000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\500x500.png" -T "image/png" -m POST http://localhost:8080/process/sepia/0,0,499,499
rem ab -v 1 -s 1 -n 50000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\500x500.png" -T "image/png" -m POST http://localhost:8080/process/grayscale/0,0,499,499
rem ab -v 1 -s 1 -n 50000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\500x500.png" -T "image/png" -m POST http://localhost:8080/process/threshold(50)/0,0,499,499
@cls
ab -v 1 -s 1 -n 5000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\zebra.png" -T "image/png" -m POST http://localhost:8080/process/rotate-cw/0,0,100,100
ab -v 1 -s 1 -n 5000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\zebra.png" -T "image/png" -m POST http://localhost:8080/process/rotate-ccw/0,0,100,100
ab -v 1 -s 1 -n 5000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\zebra.png" -T "image/png" -m POST http://localhost:8080/process/flip-v/0,0,100,100
ab -v 1 -s 1 -n 5000 -c 50 -p "../Kontur.ImageTransformer.Tests\TestData\32bit\zebra.png" -T "image/png" -m POST http://localhost:8080/process/flip-h/0,0,100,100