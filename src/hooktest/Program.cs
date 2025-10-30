using System;
using System.Reflection;
using MonoMod.RuntimeDetour;

public class Program
{
    public static void Main()
    {
        try
        {
            Console.WriteLine("=== 开始 Hook 测试 ===");

            // 检查方法是否存在
            var originalMethod = typeof(TestClass).GetMethod("OriginalMethod");
            var patchMethod = typeof(Program).GetMethod("PatchedMethod",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (originalMethod == null || patchMethod == null)
            {
                Console.WriteLine("❌ 方法获取失败！");
                return;
            }

            Console.WriteLine("✅ 方法获取成功");

            // 原始调用
            Console.WriteLine($"原始方法返回值: {TestClass.OriginalMethod()}");

            // 应用 Hook
            using (var hook = new Hook(originalMethod, patchMethod))
            {
                Console.WriteLine($"Hook 应用后返回值: {TestClass.OriginalMethod()}");
            }

            // 验证释放
            Console.WriteLine($"Hook 释放后返回值: {TestClass.OriginalMethod()}");
        }
        catch (Exception ex)
        {
            // Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❗ 异常: {ex}");
            //  Console.ResetColor();
        }
    }


    public static void TestPatch()
    {
        // 1. 原始方法调用
        int originalResult = TestClass.OriginalMethod();
        Console.WriteLine($"原始方法返回值: {originalResult} (预期: 10)");

        // 2. 应用MonoMod补丁
        var originalMethod = typeof(TestClass).GetMethod("OriginalMethod");
        var patchMethod = typeof(Program).GetMethod("PatchedMethod", BindingFlags.Static | BindingFlags.NonPublic);

        using (var hook = new Hook(originalMethod, patchMethod))
        {
            // 3. 调用补丁后的方法
            int patchedResult = TestClass.OriginalMethod();
            Console.WriteLine($"补丁后返回值: {patchedResult} (预期: 42)");

            // 4. 验证补丁
            if (originalResult != patchedResult)
            {
                Console.WriteLine("✅ 补丁验证通过 - 返回值已改变");
            }
            else
            {
                Console.WriteLine("❌ 补丁未生效 - 返回值未改变");
            }

            // 5. 额外验证方法句柄是否改变
            if (originalMethod.MethodHandle.Value != typeof(TestClass)
                .GetMethod("OriginalMethod").MethodHandle.Value)
            {
                Console.WriteLine("✅ 方法句柄已改变 - 补丁生效");
            }
        }

        // 6. 补丁作用域结束后验证恢复
        int restoredResult = TestClass.OriginalMethod();
        Console.WriteLine($"补丁释放后返回值: {restoredResult} (应恢复为: 10)");
    }

    // 补丁方法（必须使用Func<int>作为参数）
    private static int PatchedMethod(Func<int> original)
    {
        // 可以调用 original() 执行原始方法，也可以直接返回新值
        return 42; // 硬编码修改返回值
    }
}

public class TestClass
{
    public static int OriginalMethod()
    {
        return 10; // 原始返回值
    }
}
