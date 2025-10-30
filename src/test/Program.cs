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

            // 调用测试方法
            TestPatch();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❗ 异常: {ex}");
            Console.ResetColor();
        }

        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
    }

    public static void TestPatch()
    {
        // 1. 原始方法调用
        int originalResult = TestClass.OriginalMethod();
        Console.WriteLine($"原始方法返回值: {originalResult} (预期: 10)");

        // 2. 应用MonoMod补丁
        var originalMethod = typeof(TestClass).GetMethod("OriginalMethod");
        // 注意：使用正确的绑定标志获取补丁方法
        var patchMethod = typeof(Program).GetMethod("PatchedMethod",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public); // 添加 Public 标志

        if (originalMethod == null)
        {
            Console.WriteLine("❌ 无法找到原始方法");
            return;
        }

        if (patchMethod == null)
        {
            Console.WriteLine("❌ 无法找到补丁方法");
            return;
        }

        Console.WriteLine("✅ 方法获取成功");

        // 使用 Hook 类
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
        }

        // 5. 补丁作用域结束后验证恢复
        int restoredResult = TestClass.OriginalMethod();
        Console.WriteLine($"补丁释放后返回值: {restoredResult} (应恢复为: 10)");

        if (restoredResult == originalResult)
        {
            Console.WriteLine("✅ 补丁释放验证通过");
        }
        else
        {
            Console.WriteLine("❌ 补丁释放验证失败");
        }
    }

    // 补丁方法 - 修改为公共方法以便更容易访问
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